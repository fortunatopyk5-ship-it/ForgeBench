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
Fix a block factor b>1 and a Banach space B of gauge-invariant effective interactions with an exact RG map R. Assume the UV construction supplies, at the first matching scale where a chosen renormalized coupling reaches a fixed small value g_match>0, a cutoff- and volume-uniform compact set K_match subset B of possible effective actions, with matching spacing a_match comparable (up to a fixed G-dependent factor) to Lambda_YM^{-1}.

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

Statement:
At a fixed small matching coupling, convert the Balaban-type small/large-field effective density into a globally defined group-valued interaction Phi in the concrete Banach space B_{alpha,mu,p,rho} of crossover_spec.md, uniformly in UV cutoff and physical volume, with quantitative bounds

beta(Phi) in [beta_-,beta_+],
||Phi-beta(Phi)W||_{alpha,mu,p,rho} <= epsilon_match,

and with a matching-scale relation a_match Lambda_YM in [c_-,c_+].

Purpose:
Supply the actual K_match used by YM-CROSSOVER-1.

Status:
UNKNOWN.

Known input:
Balaban constructed gauge-field averaging/RG operations, developed 4D small-field effective actions and coupling renormalization, exponentiated the fluctuation integral by cluster expansion, and completed the stated 4D ultraviolet-stability program with large-field R operations.

Gap:
Those known results have NOT been verified here to imply the global Banach-ball statement above at a fixed g_match. In particular, "UV stability" is not the same statement as a compact K_match in this norm, nor a complete R^4 continuum QFT construction.

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
