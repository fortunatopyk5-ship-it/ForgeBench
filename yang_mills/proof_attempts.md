# Proof attempts

## Attempt A — spectral extraction from Euclidean-time decay
Result: successful conditional bridge (YM-BRIDGE-1).

Core identity:
C_O(t)=<psi_O,e^{-tH}psi_O>=integral_[0,infinity) e^{-tE} dmu_O(E).

A common decay rate m_* on a dense centered local algebra rules out spectral measure below m_*.

Important restriction:
The argument requires Euclidean **time** translation after reflection positivity/OS reconstruction. Generic spatial covariance decay is not automatically a spectral theorem unless Euclidean invariance lets it be placed in the reconstructed time direction.

## Attempt B — functional inequality plus locality
At strong coupling, the Shen–R. Zhu–X. Zhu proof decomposes covariance into a semigroup decorrelation term controlled by a Poincare inequality and a quasi-locality term controlled by commutators of local derivatives with the Langevin generator.

Abstract target:
Poincare rate lambda_a + derivative propagation scale v_a
=> covariance exponent gamma_a >= c lambda_a/v_a
=> require lambda_a/v_a >= c' a Lambda_YM.

New observation:
lambda_a alone is not a physically meaningful target because stochastic-quantization time can be rescaled. The ratio lambda_a/v_a is invariant under an overall rescaling of the auxiliary Markov generator and is therefore a better quantity to compare to a spatial correlation length.

Status:
PROMISING ROUTE — NOT A PROOF.

## Attempt C — compare a generic exp(-C beta) lattice bound to asymptotic scaling
For SU(N), beta_W=2N/g_0^2 and
log(a Lambda_YM) = - beta_W/(4N b_0) + O(log beta_W)
with b_0=11N/(48 pi^2).

Therefore a lower bound
gamma(beta_W) >= exp(-C beta_W+O(log beta_W))
only gives a positive continuum lower bound if its exponent/power corrections are at least as strong as the RG scale. If C>1/(4N b_0), the inferred physical lower bound gamma/a tends to zero exponentially and is useless for the Millennium gap.

Status:
PROVED as a comparison criterion, conditional on the standard asymptotic-scaling relation being the chosen trajectory.


## Attempt D — abstract semigroup-to-space theorem
Result: YM-SEMIGROUP-1 proved under explicit assumptions.

The key rate is not the Poincare constant lambda_a by itself but
  rho_a(kappa)=lambda_a/(lambda_a+v_{kappa,a}),
where v_{kappa,a} controls how quickly a local derivative can spread under the auxiliary semigroup.

For a reflection-positive lattice theory this gives the sufficient physical-gap estimate
  Delta_a >= [kappa/(2a)] rho_a(kappa).

This ratio is invariant under an overall rescaling of stochastic-quantization time L_a -> r L_a, because both lambda_a and v_{kappa,a} scale by r.

Adversarial correction:
A global PI may be stronger than necessary. Slow global/topological modes can make an auxiliary Markov gap tiny while local Euclidean correlations remain short-ranged. Future work should replace global PI by conditional/block or local-observable relaxation.

## Attempt E — RG landing into a polymer mixing domain
Instead of analytically continuing the strong-coupling proof in the bare beta, run an exact RG from weak UV coupling toward the IR.

Concrete sufficient landing condition:
at a coarse spacing a_*~Lambda_YM^{-1}, the effective polymer activities satisfy a cutoff-independent Kotecky-Preiss norm bound with exponential diameter weight.

If this single landing estimate is proved, standard cluster expansion gives exponential correlation decay at rate O(1/a_*)=O(Lambda_YM), and original-lattice reflection positivity gives the physical mass gap.

Status:
PROMISING ROUTE — NOT A PROOF.


## Attempt F — adversarial attack on YM-CROSSOVER-1

### F1. Scalar coupling is not enough
Toy RG:
R(g,r)=(g+1,r+1).
Then g->infinity while the generated remainder diverges.

Even with contraction:
R(g,r)=(g+delta,rho r+h), 0<rho<1.
If h/(1-rho) lies outside the desired cluster-expansion radius, the flow never enters that radius.

Conclusion:
the crossover must be a coupled full-action estimate, not a beta-function argument.

### F2. Measure-only KP landing is not enough
Let a fine theory split into visible variables V and hidden variables H. Let V have product measure and H have long-range correlations. An RG that retains V and integrates out H lands immediately in a product/KP coarse measure, but H-observables still have long-range correlations.

Exact identity:
Cov(F,G)
=Cov(E[F|V],E[G|V])
 + E[Cov(F,G|V)].

Therefore coarse mixing controls only the first term. A source-dependent RG locality bound is required for the second term.

Result:
YM-RG-LANDING-1 from Iteration 1 must be read with an added source/observable hypothesis (YM-RG-SOURCE-1).

## Attempt G — concrete Banach/KP specification

Defined in crossover_spec.md:
- blocks: unit 4-cells after RG rescaling;
- polymers: finite face-connected block sets;
- local variables: compact G-link variables in a one-block collar;
- local C^p norm: sup of left-invariant Lie derivatives through order p;
- global interaction norm:
  sup_B sum_{X contains B} exp(alpha|X|+mu diam(X)) ||Phi_X||_{p,rho};
- strong-coupling coordinate beta=ell_W(Phi);
- remainder r=||Phi-beta W||;
- exact gauge-covariant RG candidate via central heat-kernel blocking, exact conditional integration, rescaling, and a fixed localization prescription;
- D_KP defined by scalar super-polymer activities obtained by expanding exp(-sum Phi_X), grouping overlapping connected families, and integrating each disconnected component against product Haar.

The anchored condition
Q_{alpha,mu}(z)<alpha
is sufficient for the standard KP theorem.

## Attempt H — invariant tube

Define
K_j={beta in I_j, r<=R_j}.
If rigorous one-step enclosures bound beta' and r' into I_{j+1},R_{j+1}, induction proves
R(K_j) subset K_{j+1}.

A stronger drift form proves finite M:
beta'<=beta-delta,
r'<=rho r+B,
B<=(1-rho)R.
Then r stays in the tube and beta reaches the final region in at most
ceil((beta_max-beta_sc)/delta)+1 steps.

This removes the previous unsupported heuristic "fixed matching coupling automatically implies a finite number of nonperturbative steps".

## Attempt I — corrected lattice-to-continuum passage

Problem:
A cluster estimate may have prefactor C_O(a) diverging with cutoff, so directly passing
C_O(a)e^{-m t}
to the continuum is unsafe.

Repair:
At each fixed cutoff first use common long-time decay on a dense local sector plus reflection positivity to obtain an ACTUAL transfer spectral gap m_a. Then
C_a(t+s)<=e^{-m_a s} C_a(t).
This inequality has no observable prefactor. Positive-time correlation convergence passes it to the continuum, and the spectral theorem excludes support below liminf m_a.

Result:
YM-CUTOFF-BRIDGE-2. Equal-time norm convergence is not needed for this version.


## Attempt J — Frechet reduction of one exact RG step

For the exact block fiber integral E(Phi)(V)=-log int Q(V|U)e^{-S_Phi(U)}dU:

D E_Phi[A]=E_{Phi,V}[A],
D^2 E_Phi[A,B]=-Cov_{Phi,V}(A,B).

Combining this identity with a second-order Taylor expansion around the one-dimensional center beta W gives a finite sufficient certificate for each tube step. On I_j x [0,R_j] it is enough to bound five objects:

f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||,
A_j >= ||ell D R_{beta W}Q||,
B_j >= ||Q D R_{beta W}Q||,
C_j >= sup ||D^2 R||.

The resulting interval inequalities are written in YM-RG-TAYLOR-1.

Result:
The abstract infinite-dimensional inclusion is reduced to a finite list of scalar bounds per RG step, but obtaining those bounds for 4D Yang-Mills at intermediate coupling remains open.
