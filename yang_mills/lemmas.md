# Lemmas

## LEMMA ID: YM-BRIDGE-1
Statement:
Let (H,Omega,Hamiltonian H) be an OS-reconstructed theory with H>=0. Suppose a centered local gauge-invariant algebra produces a dense set D in Omega^perp. Assume there is a common m_*>0 such that for every psi in D there is C_psi<infinity with
<psi,e^{-tH}psi> <= C_psi e^{-m_* t}
for all sufficiently large t.
Then Spec(H)∩(0,m_*)=empty.

Purpose:
Turn Euclidean-time clustering into the spectral mass gap without treating that implication as automatic.

Dependencies:
OS reconstruction; density/cyclicity of the chosen centered algebra.

Status:
PROVED.

Proof:
For psi, spectral theorem gives
<psi,e^{-tH}psi> = integral exp(-tE) dmu_psi(E).
If mu_psi([0,m_*-epsilon])>0 for some epsilon>0, the LHS is at least
mu_psi([0,m_*-epsilon]) exp(-(m_*-epsilon)t),
contradicting the assumed upper bound as t->infinity.
Thus mu_psi([0,m_*))=0 for all psi in D. The spectral projection
P=1_[0,m_*)(H) is bounded and vanishes on dense D subset Omega^perp, hence P|Omega^perp=0.
Centering removes the vacuum atom at E=0.

Gap:
Need the hypotheses from an actual continuum Yang–Mills construction.

Counterexample search:
Spatial clustering without OS/time-translation reconstruction is not sufficient; the proof specifically uses a positive Euclidean-time semigroup.

---

## LEMMA ID: YM-CUTOFF-BRIDGE-1
Statement:
Let a_k->0 be reflection-positive lattice theories whose renormalized gauge-invariant Schwinger functions converge to an OS-reconstructible limit. If for a dense generating family the diagonal Euclidean-time correlators have a common lattice-unit decay exponent gamma(a_k) with
liminf gamma(a_k)/a_k >= m_*>0,
and the correlators converge with their t=0 norms, then the continuum Hamiltonian has gap at least m_*.

Purpose:
State the exact cutoff scaling needed from a lattice estimate.

Dependencies:
reflection positivity, convergence of renormalized correlators, YM-BRIDGE-1.

Status:
PROVED under the stated convergence/reflection-positivity hypotheses.

Proof:
At cutoff a_k (and after a volume-uniform thermodynamic estimate), reflection positivity gives a transfer Hamiltonian H_k. The common lattice-unit exponent gamma(a_k) excludes transfer spectrum below gamma(a_k), so if gamma(a_k)/a_k >= m_* then for every centered local vector psi_k,
C_k(t_k)=<psi_k,exp(-t_k H_k)psi_k> <= exp(-m_* t_k) C_k(0)
at lattice times t_k=n_k a_k.
For fixed physical t choose n_k=floor(t/a_k), hence t_k->t. By convergence of the renormalized diagonal Schwinger functions and their t=0 norms,
C(t) <= exp(-m_* t) C(0).
OS reconstruction identifies C(t)=<psi,exp(-tH)psi> in the continuum. YM-BRIDGE-1 then excludes continuum spectrum in (0,m_*), provided these local vectors are dense in Omega^perp.

Gap:
The theorem is conditional on the actual Yang-Mills hypotheses: cutoff/volume-uniform gamma(a)>=m_* a, convergence of renormalized local correlators including norms, and OS reconstruction. Those are not proved by this bridge.

Counterexample search:
If only gamma(a)>0 is known, gamma(a)/a may tend to zero. Fixed-cutoff positivity therefore does not imply a continuum mass gap.

---

## LEMMA ID: YM-IR-1
Statement:
Along a continuum Wilson trajectory beta=beta(a)->infinity, prove for some c_G>0
gamma(a) >= c_G a Lambda_YM
uniformly in volume, where gamma(a) is a common Euclidean-time exponential clustering rate in lattice units for a dense gauge-invariant local algebra.

Purpose:
This is the minimal IR estimate needed after continuum construction.

Dependencies:
none beyond the regulated lattice theory and a chosen continuum trajectory.

Status:
CONJECTURED.

Proof:
None.

Gap:
Weak-coupling, volume-uniform, cutoff-sharp correlation decay.

Counterexample search:
Any estimate gamma(a)>=exp(-C beta) is insufficient unless C and logarithmic/power corrections are quantitatively sharp relative to the RG scaling of a Lambda_YM.

---

## LEMMA ID: YM-FI-1
Statement:
For a local reversible stochastic-quantization generator L_a with invariant Wilson measure, let lambda_a be a Poincare/spectral relaxation rate and v_a a finite-range derivative-propagation scale obtained from commutators [nabla_e,L_a]. Prove a normalization-independent bound
lambda_a/v_a >= c_G a Lambda_YM
along the continuum trajectory, together with uniform locality constants sufficient to convert this ratio into Euclidean covariance decay.

Purpose:
A more attackable functional-inequality proxy for YM-IR-1.

Dependencies:
A Shen–Zhu–Zhu/Guionnet–Zegarlinski type semigroup-to-spatial-mixing theorem with cutoff-explicit constants.

Status:
CONJECTURED.

Proof:
At strong coupling an analogous mechanism is rigorous. The ratio is invariant under an overall rescaling L_a -> r_a L_a, unlike lambda_a alone.

Gap:
No weak-coupling lower bound with the required physical scaling is known here.

Counterexample search:
Bakry–Emery curvature gives no continuation: for SU(N) in d dimensions the known strong-coupling constant is
K_S=N(1/2-8|beta|(d-1)),
so in d=4 it is positive only for |beta|<1/48 and becomes negative in the continuum regime beta->infinity.

---

## LEMMA ID: YM-UV-1
Statement:
Construct a nontrivial continuum limit of 4D Wilson lattice Yang–Mills along an asymptotically-free trajectory, with renormalized gauge-invariant Schwinger functions satisfying corrected Osterwalder–Schrader hypotheses strongly enough for Wightman reconstruction.

Purpose:
Existence half of the Millennium problem.

Dependencies:
constructive RG, UV stability, thermodynamic limit, observable renormalization, Euclidean covariance restoration.

Status:
UNKNOWN.

Proof:
None in this notebook.

Gap:
Balaban-type UV control does not by itself supply the full R^4 OS theory with the IR limit and mass gap.

Counterexample search:
A subsequential or fixed-volume UV limit is insufficient if thermodynamic-limit control or nontriviality is missing.


---

## LEMMA ID: YM-SEMIGROUP-1
Statement:
Let mu be a Gibbs measure on a bounded-degree lattice, invariant for a reversible diffusion semigroup P_t with generator L. Assume:

(1) Poincare decay:
Var_mu(P_t f) <= exp(-2 lambda t) Var_mu(f).

(2) Diffusion/carre-du-champ identity:
P_t(fg)-P_t f P_t g
= 2 integral_0^t P_s Gamma(P_{t-s}f,P_{t-s}g) ds,
with Gamma decomposing into local edge gradients.

(3) Weighted gradient propagation: for every kappa>0 in some interval and local f supported on A,
G_e(P_t f) <= L_f exp(v_kappa t-kappa d(e,A)),
where G_e is the local gradient norm and v_kappa is independent of volume.

Then for local f,g with supports A,B and R=d(A,B),
|Cov_mu(f,g)| <= C_{f,g,kappa} exp(-gamma R),
with a common rate
gamma >= kappa lambda/[2(lambda+v_kappa)]
(up to an arbitrarily small loss if the underlying graph only has polynomial volume growth).

If the Euclidean lattice measure is reflection positive, the physical transfer-Hamiltonian gap in lattice units is at least this common clustering rate on the dense local gauge-invariant sector.

Purpose:
Separate the two ingredients hidden inside “functional inequality implies mass gap”: relaxation in auxiliary Markov time and finite propagation in Euclidean space.

Dependencies:
Reversible diffusion calculus; reflection positivity only for the last spectral interpretation.

Status:
PROVED under the stated abstract hypotheses.

Proof:
By invariance,
Cov(f,g)=mu(P_t(fg)-P_t f P_t g)+Cov(P_t f,P_t g).
The Poincare assumption and Cauchy-Schwarz give
|Cov(P_t f,P_t g)| <= exp(-2 lambda t) ||f-mu f||_2 ||g-mu g||_2.
For the first term, the diffusion identity and weighted gradient bound give
||P_t(fg)-P_t f P_t g||_infty
<= C L_f L_g exp(-kappa R/2+2v_kappa t).
Here one uses
sum_e exp[-kappa(d(e,A)+d(e,B))]
<= C_{kappa,d}|A||B| exp(-kappa R/2)
on a polynomial-growth lattice. Choosing
t = kappa R/[4(lambda+v_kappa)]
makes both exponents at least
kappa lambda R/[2(lambda+v_kappa)].
This proves the claim.

Gap:
At weak coupling, no required cutoff-sharp lower bound on lambda/(lambda+v_kappa) has been proved here.

Counterexample search:
A global Poincare constant may be unnecessarily strong and may be degraded by slow topological/global modes even if local physical correlations are short-ranged. Therefore the next version should seek a conditional/local or observable-restricted replacement.

---

## LEMMA ID: YM-RG-LANDING-1
Statement:
Fix a block factor b>1. Suppose an exact gauge-invariant RG can be iterated to a scale k_*(a) such that the coarse spacing
a_* = b^{k_*(a)} a
obeys c_1/Lambda_YM <= a_* <= c_2/Lambda_YM, and the exact effective measure admits a polymer representation with activities z(X) satisfying a volume- and cutoff-independent Kotecky-Preiss type bound
sup_B sum_{X contains B} ||z(X)|| exp(alpha |X| + mu diam(X)) <= epsilon < epsilon_KP
for fixed alpha,mu>0. Assume the exact RG map sends microscopic local gauge-invariant observables to quasi-local coarse observables whose tails decay faster than the same polymer rate.

Then the original lattice theory has exponential gauge-invariant correlation decay with physical rate at least c Lambda_YM, and, using reflection positivity at the ORIGINAL Wilson lattice level, its transfer Hamiltonian has a physical gap >= c Lambda_YM.

Purpose:
Turn the vague slogan “RG flows to strong coupling” into one precise landing inequality in a known mixing domain.

Dependencies:
Standard convergent polymer/cluster expansion once the KP bound holds; exact pullback of observables; YM-BRIDGE-1 / lattice transfer spectral argument.

Status:
PARTIALLY PROVED.

Proof:
The KP bound yields exponential connected-correlation decay exp(-mu R_*) in coarse lattice units for quasi-local observables. Pulling the estimate back through the exact RG gives microscopic decay exp[-c mu r/a_*]. Since a_* is comparable to Lambda_YM^{-1}, the physical decay exponent is >= c' Lambda_YM. Reflection positivity is required only for the original Wilson measure; the blocked effective action itself need not be reflection positive. The spectral theorem then gives the transfer gap.

Gap:
The landing hypothesis itself: prove that 4D asymptotically-free Yang-Mills enters such a uniform polymer domain at a physical scale O(Lambda_YM^{-1}). This is precisely the UV-to-IR crossover not controlled by present perturbative/strong-coupling estimates.

Counterexample search:
Merely reaching an O(1) effective coupling is not enough; the KP norm must actually be below its convergence threshold with cutoff-independent constants.


---

## LEMMA ID: YM-CROSSOVER-1
Statement:
Fix a block factor b>1 and a Banach space B of gauge-invariant effective interactions with an exact RG map R. Assume the UV construction supplies, at the first matching scale where a chosen renormalized coupling reaches a fixed small value g_match>0, a cutoff- and volume-uniform closed and bounded matching tube K_match subset B of possible effective actions, with matching spacing a_match comparable (up to a fixed G-dependent factor) to Lambda_YM^{-1}.

Prove that there exist an integer M_G<infinity and q<1, both independent of the original cutoff a and volume, such that
R^{M_G}(K_match) subset D_KP(q),
where D_KP(q) is a rigorously specified strong-mixing/polymer domain satisfying a Kotecky-Preiss exponential norm bound.

Purpose:
Reduce the nonperturbative UV-to-IR crossover to a FINITE number of RG steps independent of the UV cutoff. Once this inclusion holds, YM-RG-LANDING-1 follows.

Dependencies:
A UV RG package that reaches K_match uniformly and an exact RG map with controlled observable pullback.

Status:
CONJECTURED.

Proof:
No proof of the inclusion. The reduction from this inclusion to a gap is proved by YM-RG-LANDING-1.

Why finite M is plausible but not proved:
The cutoff-dependent number of RG steps occurs only while running from g_0(a)->0 to the fixed g_match. After g_match is fixed, going to any other fixed strong-coupling threshold spans a fixed logarithmic scale interval, hence a fixed number of b-blocks in the RG heuristic. Establishing the corresponding nonperturbative inclusion in the full effective-action space is exactly the missing step.

Gap:
Construct a concrete B, K_match and D_KP and validate the finite-step inclusion without projecting away relevant/irrelevant interactions or assuming monotonic coupling flow.

Counterexample search:
Tracking a single scalar coupling is insufficient: irrelevant operators generated by RG can leave the strong-mixing domain even when a nominal effective coupling is “large”. The lemma must control the full interaction norm.


---

## LEMMA ID: YM-KP-ANCHOR-1

Statement:
Let P be a hard-core polymer gas on finite connected block polymers with incompatibility given by overlap. Define

Q_{alpha,mu}(z)
 = sup_B sum_{X contains B} |z(X)| exp(alpha |X| + mu diam(X)).

If alpha,mu>0 and Q_{alpha,mu}(z) <= q < alpha, then the Kotecky-Preiss criterion holds with
a(X)=alpha |X|,
g(X)=mu diam(X).
Hence the cluster expansion converges absolutely uniformly in volume. Moreover the generalized KP tail bound has the exponential weight exp(sum g(X_i)); consequently source-connected clusters spanning supports at block distance R are bounded by C exp(-mu' R) for every fixed mu'<mu (with C depending on the fixed source supports but not total volume).

Purpose:
Give a concrete numerical threshold q_KP=alpha for the landing domain used in YM-CROSSOVER-1.

Status:
PROVED, using the standard Kotecky-Preiss theorem.

Proof:
For any polymer X0,
sum_{X incompatible with X0}
 |z(X)| exp(alpha|X|+mu diam(X))
<= sum_{B in X0} sum_{X contains B} ...
<= |X0| q
< alpha |X0| = a(X0).
This is the KP hypothesis. The weighted cluster tail statement is the second conclusion of the generalized KP theorem. A connected source cluster spanning distance R must spend total diameter at least R up to fixed source-collar corrections; reserve an arbitrarily small part of mu for those corrections.

Gap:
To apply this to YM, one must construct the scalar super-polymer activities z_Phi from the exact effective interaction and prove Q_{alpha,mu}(z_Phi)<alpha.

Counterexample search:
Failure of this sufficient KP bound does NOT imply absence of a mass gap; D_KP is a strict sufficient domain, not a characterization of all gapped theories.

---

## LEMMA ID: YM-TUBE-1

Statement:
Let beta:B->R be a bounded coordinate and r:B->[0,infinity) a remainder norm. Let
K_j={Phi: beta(Phi) in I_j=[beta_j^-,beta_j^+], r(Phi)<=R_j}.
Suppose for each j=0,...,M-1 there are rigorous enclosure functions F_j^-,F_j^+,G_j such that for all Phi in K_j,

F_j^-(beta(Phi),r(Phi))
 <= beta(R Phi)
 <= F_j^+(beta(Phi),r(Phi)),

r(R Phi) <= G_j(beta(Phi),r(Phi)),

and

inf_{I_j x [0,R_j]} F_j^- >= beta_{j+1}^-,
sup_{I_j x [0,R_j]} F_j^+ <= beta_{j+1}^+,
sup_{I_j x [0,R_j]} G_j <= R_{j+1}.

Then R(K_j) subset K_{j+1} for every j and therefore
R^M(K_0) subset K_M.

If in addition
sup_{Phi in K_M} Q_{alpha_KP,mu_KP}(z_Phi) <= q < alpha_KP,
then
R^M(K_0) subset D_KP(alpha_KP,mu_KP,q).

Purpose:
Turn YM-CROSSOVER-1 into a finite list of interval inequalities.

Status:
PROVED.

Proof:
Immediate induction on j using the three enclosure inequalities. The final inclusion is the definition of D_KP plus YM-KP-ANCHOR-1.

Gap:
The Yang-Mills one-step functions F_j^±,G_j and the final Q bound are not known.

Validated numerics:
scripts/verify_crossover_tube.py checks this induction with exact rational interval arithmetic once rigorous polynomial enclosure formulae are supplied.

---

## LEMMA ID: YM-RG-DRIFT-1

Statement:
Let
T={Phi: beta_sc <= beta(Phi) <= beta_max, r(Phi)<=R}.
Assume constants delta>0, 0<=rho<1 and B>=0 satisfy, for every Phi in T,

beta(R Phi) <= beta(Phi)-delta,

r(R Phi) <= rho r(Phi)+B,

B <= (1-rho)R.

Assume also that every Phi with beta(Phi)<=beta_sc and r(Phi)<=R belongs to D_KP.

Then any orbit starting in T reaches D_KP after at most

M <= ceil((beta_max-beta_sc)/delta)+1

steps, independently of the original UV cutoff and volume.

Purpose:
Remove the heuristic assumption M_G<infinity. Finite M follows from a uniform drift plus an invariant remainder tube.

Status:
PROVED.

Proof:
The remainder estimate and B<=(1-rho)R imply r_k<=R by induction. While beta_k>beta_sc, beta_{k+1}<=beta_k-delta. Therefore after at most the stated number of steps beta<=beta_sc. The final hypothesis gives membership in D_KP.

Gap:
The uniform positive drift delta and invariant-tube bounds are precisely the nonperturbative Yang-Mills estimates still missing.

Counterexample search:
Without an invariant remainder bound, scalar beta drift is useless; generated operators can grow and prevent KP landing.

---

## LEMMA ID: YM-CROSSOVER-TOY-OBSTRUCTION-1

Statement:
Monotone flow of a single coupling coordinate to "strong coupling" does not imply entry into any small-activity/cluster-expansion domain.

Status:
PROVED.

Proof / counterexamples:
1. R(g,r)=(g+1,r+1). Then g_k->infinity while r_k->infinity, so no domain r<epsilon is ever entered after finitely many steps.
2. Even with an irrelevant contraction,
   R(g,r)=(g+delta, rho r+h), 0<rho<1.
   If h/(1-rho)>epsilon, then r_k approaches a fixed value above epsilon. Thus contraction of the old remainder is not enough; the source term generated at each RG step must itself become small in the chosen strong-coupling coordinates.

Consequence:
YM-CROSSOVER-1 needs coupled inequalities for relevant and generated polymer/irrelevant coordinates, not only a beta function.

---

## LEMMA ID: YM-MATCH-EXTRACT-1

Statement (corrected Iteration 3):
At a fixed small matching coupling, extract from the Balaban-type UV density representation a cutoff- and volume-uniform CLOSED AND BOUNDED matching tube K_0 in a source-faithful domain-indexed activity topology. The matching datum must retain the admissible domain-history labels and separate E/R/B/large-field activities rather than first forcing them into one globally C^p interaction.

Required quantitative outputs:
(1) a coupling coordinate c(g) in a fixed interval I_0, with an explicit dictionary to the coefficient of the Wilson/classical-action direction;
(2) finite anchored activity norms for the E/R/B/large-field components, obtained from their native pointwise-decay/analyticity estimates plus a rooted tail-counting lemma;
(3) constants uniform in UV cutoff and physical volume over the selected matching scale.

Optional stronger conclusion:
If a separate embedding theorem maps this native matching space boundedly into B_{alpha,mu,p,rho}, then one may recover a global tube there. Such an embedding is NOT assumed.

Purpose:
Construct a mathematically legitimate K_0 before attempting STEP_0.

Status:
BLOCKED / REFORMULATED.

Known input:
Balaban's RG papers provide a domain-decomposed density representation, localized E/R/B terms, small-field analytic domains, and large-field/R-operation bounds in their native variables under small-coupling hypotheses.

Gap:
No theorem has been established here converting these estimates to the previous all-field global C^p remainder norm. The corrected native norm itself still needs a source-exact parameter dictionary and rooted summability constants.

Counterexample/adversarial issue:
Measure/activity suppression of large-field sectors does not imply pointwise smallness under a supremum over all gauge configurations. Sharp characteristic/domain factors cannot simply be differentiated in the old C^p norm.

---

## LEMMA ID: YM-RG-SOURCE-1

Statement:
For the exact RG used in YM-CROSSOVER-1, introduce local gauge-invariant sources J_F,J_G coupled to microscopic observables F,G. Along every eliminated shell, the first source derivatives of the effective action must remain quasi-local and the mixed second derivative must satisfy a shell-local bound whose sum over scales is exponentially decaying in the final block distance.

A sufficient schematic form is:
for sources A,B separated by R final blocks,

sum_{j=0}^{M-1}
 || d^2 S_j^eff /(dJ_A dJ_B) ||_{J=0}
 <= C_{A,B} exp(-nu R),

and the first derivatives admit quasi-local decompositions with the same or stronger exponent.

Purpose:
Transfer final coarse KP mixing back to correlations of the original microscopic local observables.

Status:
UNKNOWN.

Why necessary:
Exact RG gives the identity
Cov(F,G)
= Cov(E[F|V],E[G|V])
  + E[Cov(F,G|V)].
D_KP for the coarse measure controls only the first term. The second term is an independent conditional-covariance contribution unless source transport is controlled.

Counterexample:
Take visible coarse variables V with product measure and independent hidden variables H with long-range correlations, and let the RG discard H. The coarse measure is exactly in D_KP but H-observables remain long-range correlated.

Assessment:
This is a correction to the previous YM-RG-LANDING-1 statement. Measure landing alone is insufficient.

---

## LEMMA ID: YM-CUTOFF-BRIDGE-2

Statement:
Let a_k->0. After a thermodynamic limit at each cutoff (or a joint limit with the same uniform estimates), suppose reflection-positive lattice theories have physical transfer Hamiltonians H_k with

Spec(H_k)|_{Omega_k^perp} subset [m_k,infinity),
liminf m_k >= m_*>0.

For every member of a continuum-dense centered gauge-invariant local family, suppose renormalized positive-time diagonal correlators converge:

C_k(t)=<psi_k,e^{-tH_k}psi_k> -> C(t)
for every t>0,

and the limiting Schwinger functions are reflection positive and OS-reconstruct to
C(t)=<psi,e^{-tH}psi>.

Then the continuum Hamiltonian has no spectrum in (0,m_*).

No convergence of equal-time norms C_k(0) is required.

Status:
PROVED under the stated assumptions.

Proof:
Fix any m<m_*. For k large, m_k>=m. Hence for lattice times approximating t,s>0,

C_k(t+s) <= exp(-m s) C_k(t),

because the spectral measure of psi_k is supported in [m,infinity). Pass to the correlation-function limit:

C(t+s) <= exp(-m s) C(t).

If the continuum spectral measure of psi had positive mass in [0,m-epsilon], iterating this inequality in s would contradict the lower bound from that mass. Thus the continuum spectral measure has no support below m. Let m increase to m_* and use density of the centered local family.

Why this corrects YM-CUTOFF-BRIDGE-1:
A cluster-expansion prefactor C_O(a) may diverge as a->0. That does not matter once one first converts fixed-cutoff common exponential decay into an actual transfer spectral gap. The support inequality above survives the limit and avoids any assumption about C_k(0).

Additional assumptions made explicit:
- the fixed-cutoff clustering estimate must be common on a dense lattice local sector so that it is a true transfer gap, not an observable-specific mass;
- thermodynamic-limit control must precede or be uniform with the cutoff limit;
- limiting positive-time correlators must exist;
- reflection positivity is closed under the relevant Schwinger-function limit;
- the limiting centered local gauge-invariant states must be dense in Omega^perp;
- uniqueness of the reconstructed vacuum (or an explicit choice of vacuum sector) is required for the Clay-type statement.


---

## LEMMA ID: YM-RG-DERIV-1

Statement:
Let Q(V|U)>=0 be a normalized exact block kernel and let S_Phi(U) be a finite-volume fine action depending affinely on an interaction parameter Phi. Define the unnormalized effective action, modulo V-independent constants,

E(Phi)(V)
 = -log int Q(V|U) exp[-S_Phi(U)] dU.

For perturbations A,B of the fine action,

D E(Phi)[A](V)
 = E_{Phi,V}[A(U)],

D^2 E(Phi)[A,B](V)
 = -Cov_{Phi,V}(A(U),B(U)),

where E_{Phi,V} is expectation in the fine conditional measure with density proportional to
Q(V|U) exp[-S_Phi(U)].

Status:
PROVED.

Proof:
Differentiate the logarithm of the fiber partition function. The first derivative is the conditional expectation. Differentiating that expectation once more gives minus the connected conditional second moment. Additive V-independent normalization terms can be removed by the fixed localization/zero-mean convention.

Purpose:
Translate Frechet derivative estimates for one exact RG step into conditional moment/covariance estimates of one RG shell.

Important interpretation:
This does not assume the physical Yang-Mills mass gap. The covariance is in the constrained single-shell fluctuation measure conditioned on the retained coarse field V. To use the lemma one still needs quantitative conditional localization bounds uniform over the crossover tube.

---

## LEMMA ID: YM-RG-TAYLOR-1

Statement:
Let B be a Banach space, W in B, and ell in B* with ell(W)=1. Set
P=W ell, Q=I-P.
Write any Phi in a tube as
Phi=beta W+eta,
ell(eta)=0,
||eta||<=r.

Let R:B->B be C^2 on every segment beta W+t eta in the tube.
Define the center quantities

f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||,

and bounds

a(beta,r) >= || ell o D R_{beta W} o Q ||,
b(beta,r) >= || Q o D R_{beta W} o Q ||,
c_2(beta,r) >= sup_{0<=t<=1} ||D^2 R_{beta W+t eta}||

uniformly over ||eta||<=r.

Then

| beta(R Phi)-f(beta) |
 <= a(beta,r) r
    + (1/2)||ell|| c_2(beta,r) r^2,

and

r(R Phi)
 <= h(beta)
    + b(beta,r) r
    + (1/2)||Q|| c_2(beta,r) r^2.

Status:
PROVED.

Proof:
Second-order Banach-space Taylor formula with integral remainder, followed by ell and Q.

Purpose:
Reduce each infinite-dimensional tube inclusion to a finite collection of scalar interval bounds:
(1) center flow f,
(2) generated center remainder h,
(3) relevant/irrelevant mixing a,
(4) irrelevant amplification b,
(5) second derivative/conditional covariance c_2.

Combined with YM-RG-DERIV-1, c_2 can be attacked through conditional covariance estimates in one RG shell.

Finite enclosure form:
For beta in I_j and r<=R_j define rigorous numbers
f_j^- <= f(beta) <= f_j^+,
H_j >= h(beta),
A_j >= a(beta,R_j),
B_j >= b(beta,R_j),
C_j >= c_2(beta,R_j).
Then it suffices to choose

beta_{j+1}^-
 <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+
 >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1}
 >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

This is a finite explicit list of inequalities for each crossover step.

Gap:
No rigorous Yang-Mills values for f_j^±,H_j,A_j,B_j,C_j across the intermediate-coupling region have been established here.


---

## LEMMA ID: YM-KP-NOT-NECESSARY-1

Statement:
Membership in the particular product-Haar Kotecky-Preiss domain D_KP of crossover_spec.md is not necessary for exponential clustering.

Status:
PROVED.

Proof:
Take independent block variables with probability density proportional to exp[-V(U_B)] relative to product Haar, where V is an arbitrarily large bounded one-block potential. Distinct blocks are independent, hence every connected correlation between disjoint block observables is exactly zero. But the naive one-block activity z_B=exp[-V]-1 can have arbitrarily large sup norm, so the fixed anchored condition Q_{alpha,mu}<alpha can fail arbitrarily badly.

Consequence:
YM-CROSSOVER-1 is a sufficient strong-mixing route, not an equivalent reformulation of the mass gap. If its inclusion cannot be proved, this may reflect a poor polymer chart rather than gaplessness.

---

## LEMMA ID: YM-RG-DRIFT-2

Statement:
Let
T={Phi: beta_min <= beta(Phi) <= beta_max, r(Phi)<=R}
and let beta_min < beta_sc < beta_max.
Assume, whenever beta(Phi) in [beta_sc,beta_max],

(1) delta <= beta(Phi)-beta(R Phi) <= Delta
for constants 0<delta<=Delta;

(2) r(R Phi)<=rho r(Phi)+B,
0<=rho<1,
B<=(1-rho)R;

(3) no terminal overshoot:
if beta(Phi) in [beta_sc,beta_sc+Delta], then
beta(R Phi)>=beta_min;

(4) terminal inclusion:
{Phi: beta_min<=beta(Phi)<=beta_sc, r(Phi)<=R}
subset D_KP.

Then every orbit starting in T reaches D_KP in at most
ceil((beta_max-beta_sc)/delta)+1
steps, provided it remains in the beta interval before terminal entry.

Status:
PROVED.

Proof:
The remainder tube is invariant by (2). While beta>beta_sc, (1) decreases beta by at least delta, so after the stated number of steps it crosses beta_sc. The upper decrement bound together with (3) puts the first crossed point in the terminal strip, and (4) gives D_KP.

Purpose:
Replace the unrealistically strong terminal hypothesis in YM-RG-DRIFT-1 by a bounded terminal strip and an explicit no-overshoot condition.

---

## LEMMA ID: YM-UV-SOURCE-MATCH-1

Statement:
At the matching scale, the UV RG package must map every observable F in a chosen bounded local gauge-invariant generating algebra to a quasi-local source functional

F_match = sum_X F_X

with a common exponential localization exponent nu_UV>0 and norm
sup_B sum_{X contains B} e^{nu_UV diam(X)+alpha_UV|X|} ||F_X||_{p,rho}
<= C_F,

uniformly in UV cutoff and volume. Mixed source terms generated by eliminating UV shells must obey an analogous connected bound.

Status:
UNKNOWN.

Purpose:
Close the gap between microscopic observables and the source-dependent crossover problem. Without this, YM-RG-SOURCE-1 beginning only at K_match does not control the full original correlation function.

Known support:
Balaban's UV work provides localized effective-action/cluster-expansion machinery in the small-field regime, but the specific observable-source theorem above has not been verified here from those papers.

Gap:
Extract or prove source estimates with constants compatible with the matching Banach norm.


---

## LEMMA ID: YM-COMPACTNESS-CORRECTION-1

Statement:
Let B be an infinite-dimensional Banach space, beta in B* continuous, Q:B->B bounded, and
K={Phi in B: beta_-<=beta(Phi)<=beta_+, ||Q Phi||<=R}.
Then K is closed and bounded in the directions controlled by beta and Q (and is a bounded subset when beta together with Q controls the full norm as in Phi=beta(Phi)W+QPhi). In general K need not be compact.

For the tube-induction lemmas, compactness is unnecessary: all hypotheses may be formulated with non-attained suprema and explicit finite upper bounds.

Status:
PROVED.

Proof:
Continuity of beta and Q makes the inverse images defining K closed. In the decomposition Phi=beta(Phi)W+QPhi with beta(W)=1 and Q=I-W beta, the interval and Q-radius imply
||Phi|| <= max(|beta_-|,|beta_+|)||W||+R.
Noncompactness is generic: the closed unit ball of an infinite-dimensional normed space is not compact. YM-TUBE-1 only uses universal inequalities for all Phi in K; no maximizing Phi is required.

---

## LEMMA ID: YM-RG-CHAINRULE-1

Statement:
Let B0,B1,B2,B3 be Banach spaces,
E:B0->B1 be C^2,
S:B1->B2 bounded linear,
L:B2->B3 be C^2,
and R=L o S o E.
Then

D R_Phi
 = D L_{S E(Phi)} o S o D E_Phi,

and

D^2 R_Phi[A,B]
 = D^2 L_{S E(Phi)}
     [S D E_Phi[A], S D E_Phi[B]]
   + D L_{S E(Phi)}
     [S D^2 E_Phi[A,B]].

If L is bounded linear, the first term vanishes and
D R=L S D E,
D^2 R=L S D^2 E.

Status:
PROVED.

Proof:
Standard first- and second-order Frechet chain rule.

Purpose:
Prevent the exact fiber-integration covariance identity from being incorrectly promoted to a derivative formula for the full RG map.

Yang-Mills instantiation status:
UNKNOWN. Mapping/boundedness/differentiability of E,S,L in the corrected matching topology have not been established. In particular the localization/extraction operator L is not yet a proved bounded linear operator.

---

## LEMMA ID: YM-NATIVE-TAIL-1

Statement:
Suppose a domain-indexed polymer family F_{Sigma,X} satisfies
||F_{Sigma,X}|| <= A exp(-kappa d_Sigma(X))
and there are volume-uniform constants C_count,c_count such that for every Sigma, anchor block B and n>=0,

#{X contains B: n<=d_Sigma(X)<n+1}
 <= C_count exp(c_count n).

Then for every kappa'<kappa-c_count,

sup_{Sigma,B}
sum_{X contains B}
e^{kappa' d_Sigma(X)}
||F_{Sigma,X}||
<=
A C_count /
(1-exp[-(kappa-kappa'-c_count)]).

Status:
PROVED.

Proof:
Group polymers into integer distance shells and sum the resulting geometric series.

Purpose:
This is the missing elementary conversion between Balaban-style pointwise polymer decay and a volume-uniform anchored activity Banach norm. The nontrivial source-specific input is the rooted counting estimate and the exact comparison between Balaban's d_j and the chosen polymer index.

---

## LEMMA ID: YM-GLOBAL-CP-MATCH-IMPLICATION-1

Statement:
The currently extracted Balaban estimates do NOT establish the implication

Balaban UV/small-coupling output
=>
||Q Phi_match||_{alpha,mu,p,rho} <= epsilon

for the old global all-field C^p polymer norm.

Status:
PROVED AS A SOURCE/LOGICAL NON-IMPLICATION; NOT a theorem that such an embedding is impossible.

Reason:
The extracted hypotheses and conclusions control:
- analytic local activities on restricted regularity domains;
- sector/domain-history dependent E/R/B decompositions;
- separate large-field activities and suppression factors.

They contain no bound on derivatives of a single globally recombined interaction over all U in G^{E(X^+)} and no bounded embedding theorem into B_{alpha,mu,p,rho}. Therefore using them to infer the global norm estimate is an unsupported strengthening.

Important limitation:
It remains possible that a different argument proves the exact recombined density is globally smooth and bounded in such a norm. That theorem is simply absent from the currently extracted UV package.


---

## LEMMA ID: YM-NATIVE-POINTWISE-BANACH-1

Statement:
Fix an index set I of sector/polymer pairs i=(Sigma,X), Banach spaces H_i (for example H^infty on fixed inner analytic domains), nonnegative distances d_i, and kappa_*>0. Define

B_pt={F=(F_i): sup_i exp(kappa_* d_i)||F_i||_{H_i}<infinity}

with norm
||F||_pt=sup_i exp(kappa_* d_i)||F_i||_{H_i}.

Then B_pt is Banach.

If a source theorem gives
||F_i||_{H_i}<=A exp(-kappa d_i)
for every i and kappa>=kappa_*, then
||F||_pt<=A.

Status:
PROVED.

Proof:
The isometric map F_i -> exp(kappa_*d_i)F_i identifies B_pt with the l^infinity product of the H_i, which is complete. The source-bound implication is immediate.

Purpose:
Provide a source-faithful matching Banach topology that uses exactly the form of Balaban-type pointwise analytic polymer estimates, postponing polymer entropy/summability to YM-NATIVE-TAIL-1.

---

## LEMMA ID: YM-ANALYTIC-COLLAR-1

Statement:
Let F be holomorphic on a complex Banach-space domain D_+ and suppose every point of a smaller domain D is surrounded, in each normalized complex direction h under consideration, by a complex disk of radius delta contained in D_+. Then

||D^m F(x)[h_1,...,h_m]||
 <= m! delta^{-m} ||F||_{H^infty(D_+)}

for x in D and normalized directions, with the standard multivariable Cauchy bound.

Status:
PROVED.

Purpose:
Explain how source H^infty activity bounds on a slightly larger Balaban analytic domain can supply derivative bounds on a smaller common domain without imposing a global all-field C^p supremum.

Yang-Mills gap:
Need a uniform analytic collar delta>0 over the matching interval and sector family; this has not been extracted.

---

## LEMMA ID: YM-NATIVE-MATCH-PRESERVATION-1

Statement:
Suppose a Balaban RT/R theorem supplies a fixed inductive density predicate P_k(D) whose local E/R/B/large-field components obey pointwise analytic bounds with common decay reserve kappa>kappa_*, and suppose:
(i) RT/R maps P_k into P_{k+1};
(ii) the analytic domains contain a common inner family U^*;
(iii) the source metrics/index families admit the same pointwise norm definition at consecutive scales after rescaling.

Then the component family of every D satisfying P_k lies in a bounded ball of the pointwise native Banach space of YM-NATIVE-POINTWISE-BANACH-1, and the image lies in the corresponding next-scale bounded ball. If the amplitude bounds are unchanged by the inductive predicate, this is a self-ball preservation statement after rescaling.

Status:
PROVED CONDITIONALLY ON (i)-(iii).

Known source support:
The extracted theorem shape of CMP119 Theorem 1 / CMP122-II Theorem 1 provides (i) at the level of their native inductive density class under stated small-coupling and constant restrictions.

Unverified:
(ii)-(iii) and the full numerical parameter dictionary. Therefore this is not yet an instantiated STEP_0 theorem.


---

## LEMMA ID: YM-COMPAT-CLOSED-1

Statement:
Let B be Banach and let {C_a:B->Y_a}_{a in A} be any family of bounded linear maps into normed spaces. Then

C = intersection_{a in A} ker C_a

is a closed linear subspace of B and therefore Banach with the inherited norm.

Status:
PROVED.

Proof:
Each ker C_a is closed because C_a is continuous. An arbitrary intersection of closed sets is closed. Closed linear subspaces of Banach spaces are Banach.

Use:
Verified support/gauge/covariance/normalization/restriction equalities may be imposed this way on the native coefficient product. This does NOT prove that Balaban's complete admissibility and overlap rules are all of this form.

---

## LEMMA ID: YM-RECON-GRAPH-1

Statement:
Let X,Y be metric spaces and Rec:X->Y continuous. Then

Graph(Rec)={(x,y): y=Rec(x)}

is closed in X x Y.

If Y is a Banach function space with closed positive cone Y_+, and N:Y->R is continuous, then

{(x,rho): rho=Rec(x), rho in Y_+, N(rho)=1}

is closed.

Status:
PROVED.

Purpose:
The actual Balaban matching state should be represented as a nonlinear reconstruction graph, not as the entire linear coefficient product.

Yang-Mills instantiation:
BLOCKED because continuity of the history sum defining Rec has not been established uniformly in cutoff/volume.

---

## LEMMA ID: YM-HISTORY-SUP-FAIL-1

Statement:
There is no constant C independent of N such that for all nonnegative families
(a_1,...,a_N),

sum_{i=1}^N a_i <= C max_i a_i.

Status:
PROVED.

Proof:
Take a_i=1. Then the left side is N and the right side is C.

Consequence:
A weighted l-infinity norm over Balaban domain histories does not by itself control the reconstruction sum over histories. Either exact disjointness, cutoff-uniform finite overlap, or weighted history summability is required.

---

## LEMMA ID: YM-HISTORY-WEIGHT-1

Statement:
Let sector contributions Sec_Sigma(U) obey

|Sec_Sigma(U)| <= tau_Sigma a_Sigma

and suppose there are positive weights w_Sigma such that

a_Sigma <= R / w_Sigma

and

H_hist :=
sup_U sum_{Sigma in Adm}
 |chi_Sigma(U)| tau_Sigma / w_Sigma
< infinity.

Then

sup_U |sum_Sigma chi_Sigma(U) Sec_Sigma(U)|
<= H_hist R.

Status:
PROVED.

Proof:
Triangle inequality followed by the two assumed majorants.

Special cases:
- exact disjointness with |chi|<=1 gives H_hist<=sup tau when at most one history contributes;
- overlap multiplicity <=C gives H_hist<=C sup tau for w=1;
- genuine branching requires a nontrivial history weight.

Yang-Mills instantiation:
UNKNOWN. No source-derived disjointness theorem, cutoff-uniform overlap constant, or complete history weight has been extracted.

---

## LEMMA ID: YM-TREE-COUNT-1

Statement:
Let G be a graph of maximum degree Delta<infinity and let d be a nonnegative tree-size metric on connected finite vertex sets. Assume

|X| <= a_0 + a_1 d(X)

with constants independent of volume/scale.

Then for every anchor B and n>=0,

#{X contains B: n<=d(X)<n+1}
<= C_count exp(c_count n)

with explicit constants depending only on Delta,a_0,a_1.

Status:
PROVED.

Proof:
Fix once and for all an ordering of vertices and neighbours. Every connected m-vertex set X containing B has a canonical spanning tree (choose the first one under the induced ordering) and a canonical depth-first traversal beginning at B. The traversal has exactly 2(m-1) edge steps and its set of visited vertices is X. Hence the map X -> canonical traversal is injective. Since each step has at most Delta choices,

#{connected X containing B: |X|=m}
<= Delta^{2(m-1)}.

If d(X)<n+1, coercivity gives

m=|X| <= M_n := floor(a_0+a_1(n+1)).

Therefore

#{X contains B: n<=d(X)<n+1}
<= sum_{m=1}^{M_n} Delta^{2(m-1)}.

For Delta>1,

sum_{m=1}^{M_n} Delta^{2(m-1)}
= (Delta^{2M_n}-1)/(Delta^2-1),

so one may take, for example,

c_count=2 a_1 log Delta

and a finite C_count depending only on Delta,a_0,a_1. The cases Delta<=1 are trivial.

Balaban relevance:
The source-audit ledger identifies CMP109 d_j(X) as shortest tree-graph length divided by M for a connected finite union of localization cubes. Thus the remaining Balaban-specific geometric input is the uniform coercivity

|X| <= a_0+a_1 d_j(X)

in the exact pi_j-cube convention. The lattice-animal combinatorics itself is no longer a blocker.

---

## LEMMA ID: YM-DIAMETER-COUNT-FAIL-1

Statement:
A metric that controls only diameter does not in general imply a shell count
#{X contains B: d(X) in [n,n+1)} <= C exp(c n)
for connected subsets of Z^d when d>=2.

Status:
PROVED.

Reason:
A box of side O(n) contains O(n^d) sites. Fix a connected backbone spanning the box and independently include many adjacent optional sites while maintaining connectedness. This gives exp(c n^d) distinct connected sets of diameter O(n), which cannot be bounded by C exp(c' n).

Purpose:
The native metric must be verified to control tree/cardinality complexity, not merely geometric diameter.

---

## LEMMA ID: YM-HISTORY-POLYMER-SEPARATION-1

Statement:
A polymer shell bound for fixed history,

sup_{Sigma,B}
#{X: B in X, n<=d_Sigma(X)<n+1}
<= C exp(c n),

does not imply a corresponding bound on pairs (Sigma,X).

Status:
PROVED.

Proof:
For any fixed X one may replicate the same X under arbitrarily many distinct history labels without changing d_Sigma(X). Thus the pair count can be arbitrarily larger unless the history family has an independent multiplicity/weight bound.

Consequence:
Polymer entropy and domain-history entropy are separate certificates in YM-MATCH-EXTRACT-1.

---

## LEMMA ID: YM-NATIVE-STATE-GRAPH-1

Statement:
Let B_pt be the native coefficient Banach product, C_lin its verified closed linear compatibility subspace, I_c a coupling interval, Q a coupling-profile Banach space, and D a density Banach space. If

Rec:I_c x Q x C_lin -> D

is continuous, define

M_adm =
{(c,q,F,rho):
 c in I_c,
 q in Q,
 F in C_lin,
 rho=Rec(c,q,F),
 rho>=0,
 N(rho)=1}.

Then M_adm is a closed nonlinear subset of the product state space whenever I_c is closed, the positive cone of D is closed, and N is continuous.

Status:
PROVED CONDITIONALLY ON continuity of Rec and the chosen density topology.

Important:
M_adm is not generally linear or affine because Rec contains exponentiation/integration and positivity. Thus calling the space of actual Balaban densities a closed linear subspace of B_pt would be incorrect.

Yang-Mills instantiation:
BLOCKED by history summability/reconstruction continuity, exact admissibility data, and a fixed analytic chart.

---

## LEMMA ID: YM-RAW-COMMON-DOMAIN-WARNING-1

Statement:
Suppose a family of analytic domains has radii r_j with inf_j r_j=0. Then there is no common raw ball of positive radius delta_* contained in every domain.

Status:
PROVED.

Balaban relevance:
The extracted CMP119 domain parameters have the schematic dependence
alpha_{r,j}=g_j C_r (log g_j^{-2})^{q_r}.
For historical UV scales along an asymptotically-free trajectory, g_j may approach zero as the UV cutoff is removed. Hence a positive cutoff-uniform common raw analytic core cannot be inferred and may fail.

Repair target:
Use normalized source-dependent charts into a fixed reference domain and prove uniform chart/transition distortion instead of intersecting all raw domains.

---

## LEMMA ID: YM-LOCAL-COUPLING-PROFILE-1

Statement:
A recursion of the form

c_{j-1}(x)=c_j(x)+b_j phi_j(x)

cannot in general be represented by a single scalar coordinate c_j unless phi_j is spatially constant or the nonconstant coupling profile is stored in additional state data.

Status:
PROVED.

Proof:
If phi_j(x) is nonconstant, then even a constant c_j(x) produces a nonconstant c_{j-1}(x).

Balaban relevance:
For c_j(x)=1/g_j^2(x), CMP119's localized recursion uses phi_j supported in the relevant domain and equal to one only on an interior core. Therefore a faithful matching state requires a scalar bulk/reference c plus a localized coupling-profile coordinate (or an equivalent history-dependent field), not I_c alone.

The old ell_W coordinate remains outside the native matching construction.
